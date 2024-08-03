import {createNewPasskey, passkeyFeatureAvailable, signInPasskey, storeUserPasskey, verifyPasskey} from "./passkeys.js";

const generatePasskeyButton = document.getElementById('generate-passkey');
const passkeysAvailableSpan = document.getElementById('passkeys-available');
const signinPasskeyButton = document.getElementById('signin-passkey');

const isPasskeyFeatureAvailable = await passkeyFeatureAvailable();

passkeysAvailableSpan.textContent = isPasskeyFeatureAvailable ? 'Yes' : 'No';
generatePasskeyButton.disabled = !isPasskeyFeatureAvailable;

generatePasskeyButton.addEventListener('click', async () => {
    const passkey = await createNewPasskey();
    await storeUserPasskey(passkey);
});

signinPasskeyButton.addEventListener('click', async () => {
    const data = await signInPasskey();
    await verifyPasskey(data);
});