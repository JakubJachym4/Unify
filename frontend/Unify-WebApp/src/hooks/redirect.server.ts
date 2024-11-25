import type { Handle } from '@sveltejs/kit';
import { redirect } from '@sveltejs/kit';

export const redirectNotLogged: Handle = async ({ event, resolve }) => {
    const isLoggedIn = !!event.cookies.get('user');

    console.log(event.url.pathname)
    // If the user is not logged in and is not on the login page, redirect to login
    if (!isLoggedIn && event.url.pathname !== '/login') {
        throw redirect(302, '/login');
    }

    return resolve(event);
};
