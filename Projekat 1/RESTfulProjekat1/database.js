const mysql = require('mysql2');

module.exports = mysql.createConnection({
    host:'localhost',
    user: 'root',
    password: 'root',
    database: 'iot-project-1'
});