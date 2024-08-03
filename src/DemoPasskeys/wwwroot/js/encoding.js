export function stringToBase64(str) {
    return btoa(str);
}

export function base64ToString(base64) {
    return atob(base64);
}

export function arrayToBase64(array) {
    const uint8Array = new Uint8Array(array);
    let binaryString = "";
    for (let i = 0; i < uint8Array.length; i++) {
        binaryString += String.fromCharCode(uint8Array[i]);
    }

    return btoa(binaryString);
}

export function base64ToArray(base64String) {
    const byteCharacters = base64ToString(base64String);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    return new Uint8Array(byteNumbers);
}