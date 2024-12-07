export type ApiRequestError = Error & {
    response: Response;
};
