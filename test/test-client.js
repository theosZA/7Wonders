angular.module('testClientApp', ['angular-uuid'])
  .controller('TestClientCtrl', ["uuid", TestClientCtrl]);

function TestClientCtrl(uuid)
{
  this.userID = uuid.v4();
}