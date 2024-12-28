<script lang="ts">
    import type { MessageResponse } from '$lib/api/Messages/MessagesRequests';
    import MessageCard from './MessageCard.svelte';

    export let messages: MessageResponse[] = [];
    $: console.log('all messages: ', messages);
</script>

<div class="message-list">
    {#each messages.messages as message}
        {console.log('message list message: ', message)}
        <MessageCard {message} />
    {/each}
</div>

<style>
    .message-list {
        display: flex;
        flex-direction: column;
        gap: 1rem;
    }
</style>