const mqtt = require('mqtt')

const clientId = 'sensor_dummy'
const username = 'sensor_dummy'
const password = 'sensor_dummy'
const topic = 'sensor_dummy/values'
const qos = 2

let i = 0
data = []

const address = 'mqtt://10.66.100.154:1883'
const client = mqtt.connect(address, {
    clientId,
    username,
    password
})

const fs = require('fs')
const {parse} = require('csv-parse')
const parser = parse({columns:true}, function(err, records) {
    data = records
    setInterval(sendRow, 3000);
});

fs.createReadStream('./Preprocessed_data.csv').pipe(parser)

function sendRow() { 
    data[i].temp = parseInt(data[i].temp)
    client.publish(topic, JSON.stringify(data[i]), {qos}, error => {
        console.log("sensor_dummy salje: ", data[i])
        if (error) {
            console.error('ERROR: ', error)
        }
    })
    i = (i+1)%data.length;
}