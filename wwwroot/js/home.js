document.addEventListener('DOMContentLoaded', function() {
    const fromInput = document.getElementById('from-input');
    const fromResults = document.getElementById('from-results');
    const toInput = document.getElementById('to-input');
    const toResults = document.getElementById('to-results');
    const todayBtn = document.getElementById('btn-today');
    const tomorrowBtn = document.getElementById('btn-tomorrow');
    const dateInput = document.getElementById('date-input');


    let popularLocations = [];
    
    function updateUrlQuery() {
        const originId = fromInput.dataset.locationId || '';
        const originName = fromInput.value || '';
        const destinationId = toInput.dataset.locationId || '';
        const destinationName = toInput.value || '';
        const date = dateInput._flatpickr ? dateInput._flatpickr.input.value : '';

        const queryParams = new URLSearchParams();
        if (originId) queryParams.set('originId', originId);
        if (originName) queryParams.set('from', originName);
        if (destinationId) queryParams.set('destinationId', destinationId);
        if (destinationName) queryParams.set('to', destinationName);
        if (date) queryParams.set('date', date);

        const newUrl = `${window.location.pathname}?${queryParams.toString()}`;
        history.replaceState({ path: newUrl }, '', newUrl);
    }
    
    function renderLocationDropdown(locations, input, resultsDiv) {
        if (input === fromInput) {
            toResults.style.display = 'none';
        } else {
            fromResults.style.display = 'none';
        }
        
        resultsDiv.innerHTML = '';
        if (locations.length === 0) {
            resultsDiv.style.display = 'none';
            return;
        }

        locations.forEach(item => {
            const div = document.createElement('div');
            div.textContent = item.name;
            div.dataset.locationId = item.id;
            div.addEventListener('click', function () {
                input.value = this.textContent;
                input.dataset.locationId = this.dataset.locationId;
                resultsDiv.innerHTML = '';
                resultsDiv.style.display = 'none';
                updateUrlQuery();
            });
            resultsDiv.appendChild(div);
        });
        resultsDiv.style.display = 'block';
    }
    
    fetch('/api/locations/search')
        .then(response => response.json())
        .then(data => {
            popularLocations = data;
        })
        .catch(error => console.error('Popüler konumlar alınamadı:', error));


    function setupLocationInput(input, resultsDiv) {
        input.addEventListener('focus', function() {
            if (this.value.trim() === '' && popularLocations.length > 0) {
                renderLocationDropdown(popularLocations, input, resultsDiv);
            }
        });
        
        input.addEventListener('input', function () {
            const query = this.value;
            if (query.length < 2) {
                if (query.length === 0) {
                    renderLocationDropdown(popularLocations, input, resultsDiv);
                } else {
                    resultsDiv.style.display = 'none';
                }
                return;
            }
            fetch(`/api/locations/search?query=${encodeURIComponent(query)}`)
                .then(response => response.json())
                .then(data => {
                    renderLocationDropdown(data, input, resultsDiv);
                });
        });
    }

    setupLocationInput(fromInput, fromResults);
    setupLocationInput(toInput, toResults);
    
    document.addEventListener('click', function(e) {
        const searchCard = document.querySelector('.ticket-search-card');
        if (searchCard && !searchCard.contains(e.target)) {
            fromResults.style.display = 'none';
            toResults.style.display = 'none';
        }
    });

    function updateActiveDateButtons(selectedDate) {
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        const tomorrow = new Date(today);
        tomorrow.setDate(today.getDate() + 1);
        
        const currentDate = new Date(selectedDate);
        currentDate.setHours(0, 0, 0, 0);

        todayBtn.classList.remove('btn-active');
        tomorrowBtn.classList.remove('btn-active');

        if (currentDate.getTime() === today.getTime()) {
            todayBtn.classList.add('btn-active');
        } else if (currentDate.getTime() === tomorrow.getTime()) {
            tomorrowBtn.classList.add('btn-active');
        }
    }

    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    const initialDate = tomorrow;

    const fp = flatpickr(dateInput, {
        locale: "tr",
        dateFormat: "Y-m-d",
        altInput: true,
        altFormat: "j F Y, l",
        minDate: "today",
        defaultDate: initialDate, 
        onChange: function(selectedDates) {
            updateActiveDateButtons(selectedDates[0]);
            updateUrlQuery();
        },
        onReady: function(selectedDates, dateStr, instance) {
            updateActiveDateButtons(selectedDates[0]);
            // Sayfa yüklendiğinde mevcut query'den gelen tarih seçili değilse, URL'yi güncelle
            if (instance.input.value) {
                const urlParams = new URLSearchParams(window.location.search);
                if (urlParams.get('date') !== instance.input.value) {
                    updateUrlQuery();
                }
            }
        }
    });

    todayBtn.addEventListener('click', function() {
        fp.setDate(new Date(), true);
    });

    tomorrowBtn.addEventListener('click', function() {
        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        fp.setDate(tomorrow, true);
    });

    document.querySelectorAll('.search-input').forEach(input => {
        let timer;
        input.addEventListener('input', function() {
            clearTimeout(timer);
            const val = this.value;
            if (val.length < 2) {
                createDropdown(this, []);
                return;
            }
            timer = setTimeout(() => {
                fetch(`/api/locations/search?query=${encodeURIComponent(val)}`)
                    .then(r => r.json())
                    .then(data => createDropdown(input, data));
            }, 300);
        });
        // Kapanma için blur
        input.addEventListener('blur', function() {
            setTimeout(() => {
                const dropdown = input.parentElement.querySelector('.search-dropdown');
                if (dropdown) dropdown.style.display = 'none';
            }, 200);
        });
    });

    document.querySelector('.find-ticket-btn').addEventListener('click', function() {
        const fromInput = document.getElementById('from-input');
        const toInput = document.getElementById('to-input');
        const dateInput = document.getElementById('date-input');
        
        const originId = fromInput.dataset.locationId;
        const destinationId = toInput.dataset.locationId;
        const originName = fromInput.value;
        const destinationName = toInput.value;
        const date = dateInput._flatpickr.input.value; // API formatındaki tarihi al
    
        if (!originId || !destinationId) {
            alert('Lütfen şehirleri arama listesinden seçiniz.');
            return;
        }
    
        if (originId === destinationId) {
            alert('Kalkış ve varış noktası aynı olamaz.');
            return;
        }
    
        if (!date) {
            alert('Lütfen bir tarih seçiniz.');
            return;
        }
    
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        const selectedDate = new Date(date);
        selectedDate.setHours(0, 0, 0, 0);
    
        if (selectedDate < today) {
            alert('Geçmiş bir tarih seçilemez.');
            return;
        }
        
        const url = `/Journey/Index?originId=${originId}&from=${encodeURIComponent(originName)}&destinationId=${destinationId}&to=${encodeURIComponent(destinationName)}&date=${date}`;
        window.location.href = url;
    });
    
    document.querySelector('.swap-btn').addEventListener('click', function() {
        const fromInput = document.getElementById('from-input');
        const toInput = document.getElementById('to-input');
    
        // Metinleri değiştir
        const tempValue = fromInput.value;
        fromInput.value = toInput.value;
        toInput.value = tempValue;
    
        // ID'leri değiştir
        const tempId = fromInput.dataset.locationId;
        fromInput.dataset.locationId = toInput.dataset.locationId;
        toInput.dataset.locationId = tempId;
    
        updateUrlQuery();
    });
});

function createDropdown(input, results) {
    let dropdown = input.parentElement.querySelector('.search-dropdown');
    if (!dropdown) {
        dropdown = document.createElement('div');
        dropdown.className = 'search-dropdown';
        input.parentElement.appendChild(dropdown);
    }
    dropdown.innerHTML = '';
    if (!results || !results.length) {
        dropdown.style.display = 'none';
        return;
    }
    results.forEach(item => {
        const option = document.createElement('div');
        option.className = 'dropdown-item';
        option.textContent = item.name || item.Name;
        option.dataset.id = item.id || item.Id;
        option.onclick = () => {
            input.value = option.textContent;
            input.dataset.locationId = option.dataset.id;
            dropdown.style.display = 'none';
        };
        dropdown.appendChild(option);
    });
    dropdown.style.display = 'block';
} 