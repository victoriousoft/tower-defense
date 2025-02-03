import os
import gspread
import csv
import dotenv
import datetime

dotenv.load_dotenv()


def parse_value(value):
    if value.lower() == 'true':
        return True
    elif value.lower() == 'false':
        return False
    return value

with open("./out/combined.csv", "r", encoding="utf-8") as file:
    csv_reader = csv.reader(file)
    data = [[parse_value(cell) for cell in row] for row in csv_reader]

gc = gspread.service_account(filename=os.getenv("GOOGLE_SERVICE_ACCOUNT_PATH"))
sheet = gc.open_by_key(os.getenv("GOOGLE_SHEET_ID")).sheet1

sheet.clear()
sheet.update(values=data, range_name='A1')


