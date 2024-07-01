import requests
import json

url = "https://weatherapp-8jw4.onrender.com"


def prettyjson(d):
    return json.dumps(d, indent=4, sort_keys=True)


def log(r, d):
    response = requests.post(f"{url}/{r}", json=d)
    print(f"Status Code: {response.status_code}")
    print(prettyjson(response.json()))
    return response


data = {"address": "Jefferson, Wisconsin"}

response = log("geocode", data)

response = requests.post(f"{url}/grid", json=data)

print(f"Status Code: {response.status_code}")
print(prettyjson(response.json()))

response = requests.post(f"{url}/zone", json=data)

print(f"Status Code: {response.status_code}")
print(prettyjson(response.json()))

data = response.json()
response = requests.post(f"{url}/alerts?now", json=data)

print(f"Status Code: {response.status_code}")
print(prettyjson(response.json()))
