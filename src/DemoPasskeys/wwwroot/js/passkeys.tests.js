import {arrayToBase64, base64ToString} from './encoding.js';
import {passkeyFeatureAvailable, preparePublicKeyCredentialCreationOptions} from './passkeys.js';
import {assertEqual, assertFalse, assertTrue, it} from './test-framework.js';

await it('passkeyFeatureAvailable: should return false for http', async () => {
    const actual = await passkeyFeatureAvailable();
    assertFalse(actual);
});

await it('passkeyFeatureAvailable: should return true for https', async () => {
    const actual = await passkeyFeatureAvailable();
    assertTrue(actual);
});

it('preparePublicKeyCredentialCreationOptions: should return a PublicKeyCredentialCreationOptions object', () => {
    const source = {
        challenge: 'YWJjZGVmZw==',
        user: {name: 'irrelevant', id: 'MTIzNDU2Nzg5'}
    };

    const expectedChallenge = 'abcdefg';
    const expectedUserId = '123456789';

    const actual = preparePublicKeyCredentialCreationOptions(source);
    const actualChallenge = base64ToString(arrayToBase64(actual.challenge));
    const actualUserId = base64ToString(arrayToBase64(actual.user.id));

    assertEqual(actualChallenge, expectedChallenge);
    assertEqual(actualUserId, expectedUserId);
});