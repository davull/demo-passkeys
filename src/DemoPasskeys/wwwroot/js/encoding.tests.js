import { arrayToBase64, base64ToArray, base64ToString, stringToBase64 } from "./encoding.js";
import { assertEqual, it } from "./test-framework.js";

it("base64: should base64 encode a string", () => {
    const str = "Franz jagt im komplett verwahrlosten Taxi quer durch Bayern";
    const expected = "RnJhbnogamFndCBpbSBrb21wbGV0dCB2ZXJ3YWhybG9zdGVuIFRheGkgcXVlciBkdXJjaCBCYXllcm4=";

    const actual = stringToBase64(str);

    assertEqual(actual, expected);
});

it("base64: should base64 decode a string", () => {
    const base64 = "RnJhbnogamFndCBpbSBrb21wbGV0dCB2ZXJ3YWhybG9zdGVuIFRheGkgcXVlciBkdXJjaCBCYXllcm4=";
    const expected = "Franz jagt im komplett verwahrlosten Taxi quer durch Bayern";

    const actual = base64ToString(base64);

    assertEqual(actual, expected);
});

it("base64: should round-trip a string", () => {
    const str = "Franz jagt im komplett verwahrlosten Taxi quer durch Bayern";

    const actual = base64ToString(stringToBase64(str));

    assertEqual(actual, str);
});

it("base64 array: should base64 encode an array", () => {
    const array = new Uint8Array([0, 1, 2, 3, 4, 5, 6, 7, 8, 9]);
    const expected = "AAECAwQFBgcICQ==";

    const actual = arrayToBase64(array);

    assertEqual(actual, expected);
});

it("base64 array: should base64 decode an array", () => {
    const base64 = "AAECAwQFBgcICQ==";
    const expected = new Uint8Array([0, 1, 2, 3, 4, 5, 6, 7, 8, 9]);

    const actual = base64ToArray(base64);

    assertEqual(actual, expected);
});

it("base64 array: should round-trip an array", () => {
    const array = new Uint8Array([0, 1, 2, 3, 4, 5, 6, 7, 8, 9]);
    const actual = base64ToArray(arrayToBase64(array));

    assertEqual(actual, array);
});

it("base64 array: should round-trip with a string", () => {
    const str = "Franz jagt im komplett verwahrlosten Taxi quer durch Bayern";
    const array = base64ToArray(stringToBase64(str));
    const actual = base64ToString(arrayToBase64(array));

    assertEqual(actual, str);
});