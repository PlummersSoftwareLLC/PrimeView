var PrimeViewJS = PrimeViewJS || {};

PrimeViewJS.ShowUrl = function (url) {
	window.history.pushState(null, "", url);
};

PrimeViewJS.GetMultiselectValues = function (element, valueSeparator) {
	var selectedValues = [];

	for (var i = 0; i < element.length; i++) {
		if (element.options[i].selected)
			selectedValues.push(element.options[i].value);
	}

	return selectedValues.length > 0 ? selectedValues.join(valueSeparator) : null;
};

PrimeViewJS.ClearMultiselectValues = function (element) {
	for (var i = 0; i < element.length; i++) {
		if (element.options[i].selected)
			element.options[i].selected = false;
	}
};