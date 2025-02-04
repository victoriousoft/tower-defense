import os
import gspread
import csv
import dotenv
import datetime

dotenv.load_dotenv()

SERVICE_ACCOUNT_PATH = os.environ.get("GOOGLE_SERVICE_ACCOUNT_PATH", "./service_account.json")

if os.environ.get("GOOGLE_SERVICE_ACCOUNT"):
    with open(SERVICE_ACCOUNT_PATH, "w", encoding="utf-8") as file:
        file.write(os.environ.get("GOOGLE_SERVICE_ACCOUNT"))

def parse_value(value):
    if value.lower() == "true":
        return True
    elif value.lower() == "false":
        return False
    return value

with open("./out/combined.csv", "r", encoding="utf-8") as file:
    csv_reader = csv.reader(file)
    data = [[parse_value(cell) for cell in row] for row in csv_reader]

gc = gspread.service_account(filename=SERVICE_ACCOUNT_PATH)
spreadsheet = gc.open_by_key(os.getenv("GOOGLE_SHEET_ID"))
current_time = datetime.datetime.now().strftime("%d.%m.%Y %H:%M:%S")
sheet = spreadsheet.sheet1
sheet.update_title(f"imported {current_time}")

sheet.clear()
sheet.update(values=data, range_name='A1')


