/**
 *  Virtual Lock
 *
 */
metadata {
    definition (name: "Puppet Virtual Lock", namespace: "camthegeek", author: "Cam Soper") {
        capability "Actuator"
        capability "Lock"
		command "confirmLocked"
        command "confirmUnlocked"
    }
}

def lock() {
    log.debug "lock()"
    if (device.currentState("lock").value != "locked"){
        sendEvent(name: "lock", value: "locking", descriptionText: "${device.displayName} is locking")
    }
}

def unlock() {
    log.debug "unlock()"
    if (device.currentState("lock").value != "unlocked"){
        sendEvent(name: "lock", value: "unlocking", descriptionText: "${device.displayName} is unlocking")
    }
}

def confirmLocked(){
    sendEvent(name: "lock", value: "locked", descriptionText: "${device.displayName} is locked")
}

def confirmUnlocked(){
    sendEvent(name: "lock", value: "unlocked", descriptionText: "${device.displayName} is unlocked")
}