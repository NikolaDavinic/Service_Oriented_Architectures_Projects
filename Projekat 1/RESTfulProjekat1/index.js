const express = require('express');
const db = require('./database.js');
const os = require('os');
const network = require('network');

const path = require('path');
const bodyParser = require('body-parser');

const port = 3000;
const app = express();
app.use(express.json());

app.get("/networkLogs", async (req, res) => {
    const result = await db.promise().query('SELECT * FROM DATA');
    res.status(200).send(result);
});

app.get("/networkLogs/:id", async (req,res) => {
    const results = await db.promise().query(`SELECT * FROM DATA WHERE frame_number = ${req.params.id}`);
    res.status(200).send(results[0]);
});

app.post('/networkLogs', async (req, res) => {
    try{
        const { frame_number, frame_time, frame_len, ip_src, ip_dst, ip_len, tcp_len, value, normality } = req.body;
        if(ip_src.length > 12 || ip_dst.length > 12){
            res.status(400).send("Bad parameters!");
        }
        await db.promise().query(`INSERT INTO DATA(frame_number, frame_time, frame_len, ip_src, ip_dst, ip_len, tcp_len, value, normality) VALUE('${frame_number}', '${frame_time}', '${frame_len}', '${ip_src}', '${ip_dst}', '${ip_len}', '${tcp_len}', '${value}', '${normality}')`)
        const result = await db.promise().query(`SELECT * FROM DATA WHERE frame_number='${frame_number}'`);
        res.status(200).send(result[0]);
    }
    catch(err){
        res.status(400).send(err);
    }
});

app.put('/networkLogs', async (req,res) => {
    try{
        const { frame_number, frame_time, frame_len, ip_dst, ip_len, tcp_len, value, normality } = req.body;
        const ip_src = getLocalIP();
        const ip_src_without_dots = ip_src.replaceAll('.', '');
        await db.promise().query(`UPDATE DATA SET frame_number='${frame_number}', frame_time='${frame_time}',frame_len='${frame_len}',ip_src='${ip_src_without_dots}',ip_dst='${ip_dst}',ip_len='${ip_len}',tcp_len='${tcp_len}', value='${value}', normality='${normality}' WHERE frame_number='${frame_number}'`);
        const result = await db.promise().query(`SELECT * FROM DATA WHERE frame_number = '${frame_number}'`);
        res.status(200).send(result);
    }
    catch(err){
        res.status(400).send(err);
    }
})

app.delete('/networkLogs/:id', async (req,res) => {
    await db.promise().query(`DELETE FROM DATA WHERE frame_number = '${req.params.id}'`);
    res.status(200).send({msg: "Successfully deleted"});
});


console.log(`Network service listening on port ${port}`);
console.log(`${getLocalIP()}`);
app.listen(port);

function getLocalIP() {
    const interfaces = os.networkInterfaces();
    for (const interfaceName in interfaces) {
      const iface = interfaces[interfaceName];
      for (const alias of iface) {
        if (alias.family === 'IPv4' && alias.address !== '127.0.0.1' && !alias.internal) {
          return alias.address;
        }
      }
    }
    return 'Unable to retrieve local IP address';
  }