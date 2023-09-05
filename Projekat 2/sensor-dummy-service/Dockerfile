FROM node:alpine
WORKDIR /usr/src/app
COPY ./sensor-dummy-service/package*.json .
RUN npm install
COPY ./sensor-dummy-service .
EXPOSE 3000
CMD ["node", "index.js"]