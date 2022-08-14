var PrimeViewJS = PrimeViewJS || {};

PrimeViewJS.ShowUrl = function (url) {
	window.history.replaceState(null, "", url);
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

PrimeViewJS.SetMultiselectValues = function (element, values) {
	for (var i = 0; i < element.length; i++) {
		element.options[i].selected = values.includes(element.options[i].value);
	}
};

PrimeViewJS.DownloadFileFromStream = async function (fileName, mimeType, contentStreamReference) {
	const arrayBuffer = await contentStreamReference.arrayBuffer();
	const blob = new Blob([arrayBuffer], { type: mimeType });
	const url = URL.createObjectURL(blob);
	const anchorElement = document.createElement('a');
	anchorElement.href = url;
	anchorElement.download = fileName ?? '';
	anchorElement.click();
	anchorElement.remove();
	URL.revokeObjectURL(url);
}

var blinkTexts = [];
var blinkCount = 0;

function blinkInit () {
    if (!$("#blinkTextMessage"))
        return false;

    $(".blinkTextContent").each((_index, element) => {
		blinkTexts.push($(element).text());
    });

	return blinkTexts.length > 0;
}

function blinkText () {
    var blinkTextDiv = $("#blinkTextMessage");
    if(!blinkTextDiv)
		return;

	if (blinkCount >= blinkTexts.length)
		blinkCount = 0;

	blinkTextDiv.html(blinkTexts[blinkCount++]);
	blinkTextDiv.fadeIn(300).animate({ opacity: 1.0 }).fadeOut(300, () => blinkText());
}

if (blinkInit())
	blinkText();
