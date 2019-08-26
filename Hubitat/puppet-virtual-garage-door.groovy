/**
 *  Virtual Garage Door
 *
 */
metadata {
    definition (name: "Puppet Virtual Garage Door", namespace: "camthegeek", author: "Cam Soper") {
		capability "Contact Sensor" 
        capability "Garage Door Control"
        command "confirmOpen"
        command "confirmClosed"
    }
}

def open() {
    log.debug "open()"
    if (device.currentState("door").value != "open"){
        sendEvent(name: "door", value: "opening")
    }
}

def close() {
    log.debug "close()"
    if (device.currentState("door").value != "closed"){
        sendEvent(name: "door", value: "closing")
    }
}


def confirmClosed(){
	sendEvent(name: "door", value: "closed")
    sendEvent(name: "contact", value: "closed")
}

def confirmOpen(){
    sendEvent(name: "door", value: "open")  
    sendEvent(name: "contact", value: "open")
}