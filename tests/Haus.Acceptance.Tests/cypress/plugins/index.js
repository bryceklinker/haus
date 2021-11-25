/// <reference types="cypress" />
// ***********************************************************
// This example plugins/index.js can be used to load plugins
//
// You can change the location of this file or turn off loading
// the plugins file with the 'pluginsFile' configuration option.
//
// You can read more here:
// https://on.cypress.io/plugins-guide
// ***********************************************************

// This function is called when a project is opened or re-opened (e.g. due to
// the project's config changing)

/**
 * @type {Cypress.PluginConfig}
 */
const {connect} = require('mqtt');

let mqttClient = null;

function getMqttClient(config) {
    if (mqttClient) {
        return mqttClient;
    }
    
    return mqttClient = connect(config.env['MQTT_SERVER'])
}

module.exports = (on, config) => {
  on('task', {
      publishToMqtt({topic, json}) {
          return new Promise((resolve, reject) => {
              const client = getMqttClient(config);
              client.publish(topic, json, (err) => {
                if(err) {
                    reject(err);
                } else {
                    resolve(null);
                }
              });
          });
      }
  })
}
