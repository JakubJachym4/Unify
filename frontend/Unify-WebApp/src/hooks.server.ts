export { redirectNotLogged } from './hooks/redirect.server'
import type { Handle } from '@sveltejs/kit'
import { redirectNotLogged } from './hooks.server';

const chainHooks = (...hooks: Handle[]): Handle => {
    return async ({ event, resolve }) => {
        let currentResolve = resolve

        // Process hooks in reverse order to ensure the first hook is called last
        for (const hook of hooks.reverse()) {
            const previousResolve = currentResolve;
            currentResolve = async (event) => hook({ event, resolve: previousResolve });
        }

        return currentResolve(event);
    };
};

// Combine hooks into a single `handle` function
export const handle: Handle = chainHooks(redirectNotLogged);
