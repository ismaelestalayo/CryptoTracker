
import requests
import json

URL = "https://pro-api.coinmarketcap.com/v1/cryptocurrency/map"
API_KEY = "KEY"

hdrs = {"X-CMC_PRO_API_KEY": API_KEY}
resp = requests.get(URL, headers=hdrs)

if (resp.ok):
    data = json.loads(resp.content)["data"]

    coins = sorted(data, key=lambda d: int(d["rank"]))
    coins = coins[:800]

    KEYS = ["id", "name", "symbol", "rank"]
    coins = [{k: v for k, v in c.items() if k in KEYS} for c in coins]

    with open("CoinList.json", "w", encoding="utf8") as f:
        json.dump(coins, f, sort_keys=False, indent=4)

    print("> Top cryptos fetched.")

else:
    print("> Error on the GET request...")
