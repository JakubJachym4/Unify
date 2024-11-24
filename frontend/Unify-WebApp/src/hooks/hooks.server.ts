import type { Handle } from '@sveltejs/kit';

export const handle: Handle = async ({ event, resolve }) => {
    const isLoggedIn = !!event.cookies.get('user');

    // If the user is not logged in and is not on the login page, redirect to login
    if (!isLoggedIn && event.url.pathname !== '/login') {
        return Response.redirect('/login', 302);
    }

    return resolve(event);
};
