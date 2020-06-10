import {parseQueryString} from "./query-string-parser";

test('when no query string then returns empty map', () => {
    const result = parseQueryString('https://something.com');
    
    expect(result.size).toEqual(0);
});

test('when one query parameter then returns map with one entry', () => {
    const result = parseQueryString('https://something.com?bob=six');
    
    expect(result.get('bob')).toEqual('six');
});

test('when query parameter is url encoded then returns map with url decoded value', () => {
    const result = parseQueryString(`https://something.com?redirect_url=${encodeURIComponent('https://localhost:5001')}`)
    
    expect(result.get('redirect_url')).toEqual('https://localhost:5001');
})