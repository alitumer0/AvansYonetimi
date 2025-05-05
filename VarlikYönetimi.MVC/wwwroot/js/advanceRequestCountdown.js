function initializeCountdown(createdAt, status) {
    var timeLimits = {
        'BMOnayBekliyor': 24,
        'DirektorOnayBekliyor': 24,
        'GMYOnayBekliyor': 24,
        'GMOnayBekliyor': 24,
        'FMOdemeTarihiBelirleme': 48,
        'OdemeHazir': 48,
        'AvansGeriOdenmeyiBekliyor': 720,
        'HukukiIslemBaslatildi': 360
    };

    var timeLimit = timeLimits[status] || 24; // Varsayılan 24 saat

    function updateCountdown() {
        var now = new Date();
        var elapsedHours = (now - new Date(createdAt)) / (1000 * 60 * 60);
        var remainingHours = Math.max(0, timeLimit - elapsedHours);
        
        var days = Math.floor(remainingHours / 24);
        var hours = Math.floor(remainingHours % 24);
        var minutes = Math.floor((remainingHours - Math.floor(remainingHours)) * 60);
        
        var timeString = '';
        if (days > 0) {
            timeString += days + ' gün ';
        }
        if (hours > 0 || days > 0) {
            timeString += hours + ' saat ';
        }
        timeString += minutes + ' dakika';
        
        if (remainingHours <= 0) {
            timeString = "Süre doldu!";
            $('#countdown').removeClass('text-warning').addClass('text-danger');
        }
        
        $('#timeRemaining').text(timeString);
    }

    $(document).ready(function() {
        updateCountdown();
        setInterval(updateCountdown, 60000);
    });
} 