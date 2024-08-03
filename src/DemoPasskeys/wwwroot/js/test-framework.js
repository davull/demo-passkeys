export async function it(description, sut) {
    const result = document.createElement('p');
    result.classList.add('test');

    try {
        await sut();

        result.classList.add('success');
        result.textContent = description;
    } catch (error) {
        result.classList.add('failure');
        result.textContent = `${description} (${error.message})`;
    }

    document.body.appendChild(result);
}

export function assertEqual(x, y) {
    if (x === y || (
        typeof x === 'object' &&
        typeof y === 'object' &&
        x.length === y.length &&
        x.every((element, index) => element === y[index])))
        return;

    throw new Error(`${x} != ${y}`);
}

export function assertTrue(x) {
    assertEqual(x, true);
}

export function assertFalse(x) {
    assertEqual(x, false);
}