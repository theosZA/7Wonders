angular.module('testClientApp', ['angular-uuid'])
  .controller('TestClientCtrl', ["uuid", TestClientCtrl]);

function TestClientCtrl(uuid)
{
  this.generateUserId = function ()
  {
    this.userId = uuid.v4();
  }

  this.generateUserId();
}