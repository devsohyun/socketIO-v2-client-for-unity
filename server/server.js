const express = require('express')
const app = express()
const http = require('http').Server(app)
const io = require('socket.io')(http)
const port = 8888

//server
http.listen(port, function(){
  console.log('listening on :', port)
})

//io
io.on('connect', (socket) => {
  //on connect
  console.log(`socket is connecting`)

  // from unity
  socket.on('KnockKnock', (data) => {
    console.log(`game is ready to send user name`)
  })

  socket.on('ThisIsData', (data) => {
    let userName = data.name
    console.log('user name is: ' + userName)

    socket.emit('Reaction')
  })
  //on disconnect
  socket.on('disconnect', function() {
    console.log('disconnect', socket.id)
  })

})
