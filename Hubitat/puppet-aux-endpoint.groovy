definition(
    name: "Puppet Auxilliary Endpoint",
    namespace: "camthegeek",
    author: "Cam Soper",
    description: "Puppet Auxilliary Endpoint",
    category: "",
    iconUrl: "https://s3.amazonaws.com/smartapp-icons/Convenience/Cat-Convenience.png",
    iconX2Url: "https://s3.amazonaws.com/smartapp-icons/Convenience/Cat-Convenience@2x.png",
    iconX3Url: "https://s3.amazonaws.com/smartapp-icons/Convenience/Cat-Convenience@2x.png")


preferences {
  page(name: "setupScreen")
}
def setupScreen(){
   if(!state.accessToken){	
       createAccessToken() //be sure to enable OAuth in the app settings or this call will fail
   }
   return dynamicPage(name: "setupScreen", uninstall: true, install: true){
       section ("Notify these phones...") {
            input "phones", "capability.notification", multiple: true, required: true
      }
       section(){ 
           paragraph("Access token: ${state.accessToken}")
       }
   }
}

mappings {
  path("/notify") {
    action: [
      POST: "notifyPhones"
    ]
  }
  path("/suntimes") {
    action: [
      GET: "getSunTimes"
    ]
  }  
}

def getSunTimes() {
    return getSunriseAndSunset()
}

void notifyPhones() {
    def notificationtext = request.JSON?.notificationText
    phones.deviceNotification(notificationtext)
}
def installed() {}

def updated() {}