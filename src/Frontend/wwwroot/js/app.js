var PrimeViewJS = PrimeViewJS || {};

PrimeViewJS.ShowUrl = function (url) {
	window.history.pushState(null, "", url);
}