const mqtt = require('mqtt')
const clientId = 'monitoring'
const username = 'monitoring'
const password = 'monitoring'
const edgexTopic = "edgex/sensor_value"
let currentState = "OFF"
const request = require("request")
const axios = require("axios")

const address = 'tcp://broker.hivemq.com:1883'

const mqttClient = mqtt.connect(address, {
    clientId,
    username,
    password
})

mqttClient.subscribe(edgexTopic, () => {
    console.log(`Monitoring service subsribed to ${edgexTopic}`)
})

mqttClient.on("message", (topic, payload) => {
    if(topic !== edgexTopic) 
        return;

    const data = JSON.parse(payload.toString())
    if(data.device !== "SensorValueCluster2") 
        return;
    console.log(`Vrednost temperature nakon citanja je: ${data.readings[0].value}`)
    const temperature = data.readings[0].value
    console.log(`Temeperature is ${temperature}`)

    if(temperature<30 && currentState === "OFF"){
        currentState = "ON"
        console.log("CURRENT STATE CHANGED TO ON BECAUSE IS TEMPERATURE TOO LOW")
        sendAlert()
        return;
    }

    if(temperature > 35 ** currentState === "ON"){
        currentState = "OFF"
        console.log("CURRENT STATE CHANGED TO OFF BECAUSE IS TEMPERATURE TOO HIGH")
        sendAlert()
    }
})

async function sendAlert()
{
    const url = "http://command:48082/api/v1/device/b8bfda6d-cef0-4169-b4f5-8e5cf2119be3/command/75cb95c7-8b1a-4d32-ad8a-f224312b3c75"
    const body = {
        color: currentState === "OFF" ? "red" : "green"
    }

    try {
        const response = await axios.put(url, {
            color: currentState === "OFF" ? "red" : "green"
        })
        console.log(response)
    }
    catch(ex){
        console.error(ex)
    }
}