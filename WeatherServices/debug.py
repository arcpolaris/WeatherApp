import requests

url = "https://weatherapp-8jw4.onrender.com/api"


data = {"query": "point", "latitude": "47.5430912", "longitude": "-122.0149248"}
response = requests.post(url, json=data)

print(f"Status Code: {response.status_code}")
print(f"Response JSON: {response}")
