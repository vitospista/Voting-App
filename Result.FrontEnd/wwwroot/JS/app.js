const host = "http://localhost:7071/api/";

const getStandings = (callback) =>  $.getJSON(host + "GetStandings", callback);
const getLastVote = (callback) => $.getJSON(host + "GetLastVote", callback);

var ViewModel = function () {
    var self = this;
    self.standings = ko.observable({});
    self.lastVote = ko.observable({});
    self.choiceA = ko.pureComputed(function () {
        return self.standings().a;
    })
    self.choiceB = ko.pureComputed(function () {
        return self.standings().b;
    });
    self.total = ko.pureComputed(function () {
        return self.choiceA() + self.choiceB()
    });

    self.voteCount = ko.pureComputed(function () {
        let voteCount = self.total() || 0;
        if (voteCount === 0) return 'No votes yet';
        else if (voteCount === 1) return '1 vote';
        else return `${voteCount} votes`;
    });

    self.getLastVote = () => getLastVote(data => { console.log(data); self.lastVote(data) });

    self.init = () => getStandings(data => self.standings(data));    

    self.init();
    self.getLastVote();   
};

ko.applyBindings(new ViewModel());

const LOCAL_BASE_URL = 'http://localhost:7071';
const connect = () => {
    const connection = new signalR.HubConnectionBuilder().withUrl(`${LOCAL_BASE_URL}/api`).build();

    connection.onclose(() => {
        console.log('SignalR connection disconnected');
        setTimeout(() => connect(), 2000);
    });

    connection.on('voteInserted', data => {
        console.log(data)
    });

    connection.start().then(() => {
        console.log("SignalR connection established");
    });
};

connect();