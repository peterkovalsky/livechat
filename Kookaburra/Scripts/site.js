function showError(errorMessage) {
    $('#global-alert strong').html(errorMessage);
    $('#global-alert').show();
}

function isNullOrWhitespace(input) {
    if (typeof input === 'undefined' || input == null) return true;

    return input.replace(/\s/g, '').length < 1;
}