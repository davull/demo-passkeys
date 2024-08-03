import {arrayToBase64, base64ToArray} from "./encoding.js";

export async function passkeyFeatureAvailable() {
    if (window.PublicKeyCredential &&
        PublicKeyCredential.isUserVerifyingPlatformAuthenticatorAvailable &&
        PublicKeyCredential.isUserVerifyingPlatformAuthenticatorAvailable) {

        const b1 = await PublicKeyCredential.isUserVerifyingPlatformAuthenticatorAvailable();
        const b2 = await PublicKeyCredential.isConditionalMediationAvailable();

        return b1 && b2;
    }

    return false;
}

// https://web.dev/articles/passkey-registration
// https://developer.mozilla.org/en-US/docs/Web/API/Web_Authentication_API
export async function createNewPasskey() {
    const publicKeyCredentialCreationOptions = await getPublicKeyCredentialCreationOptions();
    const credentials = await navigator.credentials.create({publicKey: publicKeyCredentialCreationOptions});
    const response = credentials.response;

    return {
        // id: A Base64URL encoded ID of the created passkey. This ID helps the browser to determine whether a matching
        // passkey is in the device upon authentication. This value needs to be stored in the database on the backend.
        id: credentials.id,
        userId: arrayToBase64(publicKeyCredentialCreationOptions.user.id),
        publicKey: arrayToBase64(response.getPublicKey()),
        publicKeyAlgorithm: response.getPublicKeyAlgorithm(),
        transports: response.getTransports()
    };
}

export async function storeUserPasskey(data) {
    await fetch('/api/passkeys/userpasskey', {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(data),
    });
}

// https://developer.mozilla.org/en-US/docs/Web/API/CredentialsContainer/get#web_authentication_api
// https://w3c.github.io/webauthn/#iface-authenticatorassertionresponse
export async function signInPasskey() {
    const publicKeyCredentialRequestOptions = await getPublicKeyCredentialRequestOptions();
    const publicKeyCredential = await navigator.credentials.get({publicKey: publicKeyCredentialRequestOptions});
    const response = publicKeyCredential.response;

    return {
        id: publicKeyCredential.id,
        authenticatorData: arrayToBase64(response.authenticatorData),
        clientDataJson: new TextDecoder().decode(response.clientDataJSON),
        signature: arrayToBase64(response.signature),
        userHandle: arrayToBase64(response.userHandle)
    };
}

export async function verifyPasskey(data) {
    await fetch('/api/passkeys/verify', {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(data),
    });
}

async function getPublicKeyCredentialCreationOptions() {
    const response = await fetch('/api/passkeys/PublicKeyCredentialCreationOptions');
    return preparePublicKeyCredentialCreationOptions(await response.json());
}

async function getPublicKeyCredentialRequestOptions() {
    const response = await fetch('/api/passkeys/PublicKeyCredentialRequestOptions');
    return preparePublicKeyCredentialRequestOptions(await response.json());
}

export function preparePublicKeyCredentialCreationOptions(source) {
    return {
        ...source,
        challenge: base64ToArray(source.challenge),
        user: {
            ...source.user,
            id: base64ToArray(source.user.id)
        }
    }
}

export function preparePublicKeyCredentialRequestOptions(source) {
    return {
        ...source,
        challenge: base64ToArray(source.challenge),
    }
}