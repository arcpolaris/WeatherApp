import requests, json

url = "https://weatherapp-8jw4.onrender.com/"


def prettyjson(d):
    return json.dumps(d, indent=4, sort_keys=True)


data = {"lat": 47.5430912, "lon": -122.0149248}
response = requests.post(url + "map", json=data)

print(f"Status Code: {response.status_code}")
print(prettyjson(response.json()))
