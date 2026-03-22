<script lang="ts">
  import Login from '$lib/Login.svelte';
  import Dashboard from '$lib/Dashboard.svelte';
  
  let token = $state('');
  
  function handleLogin(event: CustomEvent<string>) {
    token = event.detail;
  }
  
  function handleLogout() {
    token = '';
  }
</script>

<svelte:head>
  <title>Dealer Portal | Oracle CarStock</title>
</svelte:head>

<main>
  <div class="logo-container">
    <h1>CarStock <span class="badge">Portal</span></h1>
  </div>

  {#if !token}
    <Login on:login={handleLogin} />
  {:else}
    <Dashboard {token} on:logout={handleLogout} />
  {/if}
</main>

<style>
  main {
    max-width: 1000px;
    margin: 0 auto;
  }
  
  .logo-container {
    text-align: center;
    margin-bottom: var(--spacing-6);
  }
  
  h1 {
    font-size: var(--font-size-2xl);
    display: inline-flex;
    align-items: center;
    gap: var(--spacing-3);
  }
  
  .badge {
    background: var(--color-secondary);
    color: var(--color-text);
    font-size: var(--font-size-base);
    padding: var(--spacing-1) var(--spacing-3);
    border: 2px solid var(--color-text);
    border-radius: var(--radius-sm);
    box-shadow: 2px 2px 0px var(--color-text);
    transform: rotate(-3deg);
  }
</style>
