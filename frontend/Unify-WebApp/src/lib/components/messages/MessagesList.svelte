<script lang="ts">
    import type { MessageResponse } from '$lib/api/Messages/MessagesRequests';
    import MessageCard from './MessageCard.svelte';

    export let messages: { messages: MessageResponse[] } = { messages: [] };
    
    $: sortedMessages = messages.messages.sort((a, b) => 
        new Date(b.createdOn).getTime() - new Date(a.createdOn).getTime()
    );
</script>

<div class="message-list">
    {#each sortedMessages as message}
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