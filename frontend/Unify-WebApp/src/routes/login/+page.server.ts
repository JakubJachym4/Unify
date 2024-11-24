import type { Actions } from './$types';

export const actions: Actions = {
    login: async ({ cookies, request }) => {
        const formData = await request.formData();
        const email = formData.get('email');
        const password = formData.get('password');

        if (email === 'user@example.com' && password === 'password') {
            cookies.set('user', JSON.stringify({ id: '1', name: 'John Doe', email }), {
                path: '/',
                httpOnly: true,
                sameSite: 'strict',
            });
            return { success: true };
        }

        return { success: false, error: 'Invalid email or password.' };
    },
};
