fetch('SirenData.json')
    .then((response) => response.json())
    //.then((json) => console.log(json));
    .then((json) => {
        let countyEl = document.getElementById("counties");
        let cityEl = document.getElementById("cities");
        Object.keys(json.cities).forEach((county) =>
            countyEl.appendChild(Object.assign(document.createElement('option'), { value: county, textContent: county }))
        );
        countyEl.addEventListener('change', () => {
            let cities = json.cities[countyEl.value] || [];
            cityEl.innerHTML = '';
            cities.forEach((city) => {
                cityEl.appendChild(Object.assign(document.createElement('option'), { value: city, textContent: city }));
            });
        });
    });
document.getElementById("form").addEventListener('submit', (e) => {
    e.preventDefault();

    fetch('https://weatherapp-8jw4.onrender.com/publish', {
        method: 'POST',
        mode: 'no-cors',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            title: "Siren Alert",
            subtitle: document.getElementById("alert-types").value,
            description: document.getElementById("comment").value,
            county: document.getElementById("counties").value,
            city: document.getElementById("cities").value
        })
    })
});