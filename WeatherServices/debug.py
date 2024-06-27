import requests, json

url = "https://weatherapp-8jw4.onrender.com"


def prettyjson(d):
    return json.dumps(d, indent=4, sort_keys=True)


data = {}
response = requests.post(f"{url}/alerts", json=data)

print(f"Status Code: {response.status_code}")
print(prettyjson(response.json()))
