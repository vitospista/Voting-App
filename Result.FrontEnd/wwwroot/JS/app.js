const standingsUrl = "http://localhost:7071/api/GetStandings";


var ViewModel = function () {
    var self = this;
    self.standings = ko.observable({});
    self.choiceA = ko.pureComputed(function () {
        return self.standings().a;
    });
    self.choiceB = ko.pureComputed(function () {
        return self.standings().b;
    });
    self.total = ko.pureComputed(function () {
        return self.choiceA() + self.choiceB()
    })

    self.voteCount = ko.pureComputed(function () {
        let voteCount = self.total() || 0;
        if (voteCount === 0) return 'No votes yet';
        else if (voteCount === 1) return '1 vote';
        else return `${voteCount} votes`;
    })

    self.init = function () {
        $.getJSON(standingsUrl, function (data) {
            self.standings(data);
        });
    }
    self.init();
};

ko.applyBindings(new ViewModel());