function convertQueryStringToObject(queryString) {
    const pairs = queryString.split('&');
    return pairs
        .map(pair => {
            const key = pair.split('=')[0];
            const value = pair.split('=')[1];
            return {[key]: decodeURIComponent(value)};
        })
        .reduce((result, value) => ({...result, ...value}), {});
}

function missingQueryString(url) {
    return url.indexOf('?') < 0;
}

export function parseQueryString(url) {
    if (missingQueryString(url)) {
        return new Map();
    }
    const queryString = url.substring(url.indexOf('?') + 1);
    const json = convertQueryStringToObject(queryString);
    return new Map(Object.entries(json));
}