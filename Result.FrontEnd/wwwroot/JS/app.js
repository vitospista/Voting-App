//const host = "http://localhost:7071/api/";
const host = "https://voting-app-backend.azurewebsites.net/api/";
const getStandings = (callback) =>  $.getJSON(host + "GetStandings", callback);
const getLastVote = (callback) => $.getJSON(host + "GetLastVote", callback);

var ViewModel = function () {
    var self = this;
    self.choiceA = ko.observable(0);
    self.choiceB = ko.observable(0);
    self.total = ko.pureComputed(function () {
        return self.choiceA() + self.choiceB()
    });

    self.voteCount = ko.pureComputed(function () {
        let voteCount = self.total() || 0;
        if (voteCount === 0) return 'No votes yet';
        else if (voteCount === 1) return '1 vote';
        else return `${voteCount} votes`;
    });

    self.increment = function(vote){
        switch (vote.choice) {
            case 'a':
                self.choiceA(self.choiceA() + 1);
                break;
            case 'b':
                self.choiceB(self.choiceB() + 1);
                break;
            default:
                console.log(vote);
        }
    }

    self.init = () => getStandings(data => {
        self.choiceA(data.a);
        self.choiceB(data.b);
    });    

    self.init();
};

let vm = new ViewModel(); 
ko.applyBindings(vm);

const connect = (vm) => {
    const connection = new signalR.HubConnectionBuilder().withUrl(`${host}`).build();

    connection.onclose(() => {
        console.log('SignalR connection disconnected');
        setTimeout(() => connect(), 2000);
    });

    connection.on('voteInserted', data => {
        console.log(data);
        vm.increment(data);
    });

    connection.start().then(() => {
        console.log("SignalR connection established");
    });
};

connect(vm);