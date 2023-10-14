import requests
import json
import random
import time
import csv

i = 0

if __name__ == "__main__":

    with open("D:\Fakultet\Soa\Service_Oriented_Architectures_Projects\Projekat 3\data-reader-service\iot3data.csv", 'r') as file:
        csvreader = csv.reader(file) 
        for row in csvreader:
            
            if i == 0: 
                i = i + 1
                continue

            url = 'http://localhost:49986/api/v1/resource/SensorValueCluster2/temperature'
            payload = int(float(row[3]))
            headers = {'content-type': 'application/json'}
            response = requests.post(url, data=json.dumps(payload), headers=headers, verify=False)
            print(response)

            time.sleep(2)