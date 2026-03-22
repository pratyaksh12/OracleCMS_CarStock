<script lang="ts">
  import { createEventDispatcher } from 'svelte';
  
  let error = $state('');
  let username = $state('');
  let password = $state('');
  const dispatch = createEventDispatcher();

  async function handleLogin(e: Event) {
    e.preventDefault();
    try {
      const res = await fetch('http://localhost:5237/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ Username: username, Password: password })
      });
      
      if (!res.ok) {
        error = 'Login failed. Please check your credentials.';
        return;
      }
      
      const data = await res.json();
      // FastEndpoints JWT generator returns token in lowercase 'token' by default
      dispatch('login', data.token || data.Token); 
    } catch (err: any) {
      error = err.message || 'An error occurred during login.';
    }
  }
</script>

<div class="login-container">
  <h2>Dealer Login</h2>
  <p class="subtitle">Access your CarStock inventory</p>

  {#if error}
    <div class="alert danger" role="alert">{error}</div>
  {/if}
  
  <form onsubmit={handleLogin}>
    <div class="form-group">
      <label for="username">Username</label>
      <input id="username" bind:value={username} placeholder="Enter your username" required />
    </div>
    
    <div class="form-group">
      <label for="password">Password</label>
      <input type="password" id="password" bind:value={password} placeholder="Enter your password" required />
    </div>
    
    <button type="submit" class="btn primary">Login / Create Account</button>
  </form>
</div>

<style>
  .login-container {
    max-width: 420px;
    margin: var(--spacing-8) auto;
    padding: var(--spacing-6);
    background: var(--color-surface);
    border: 3px solid var(--color-text); /* High contrast border */
    border-radius: var(--radius-lg);
    box-shadow: 8px 8px 0px var(--color-text); /* Brutalist shadow */
  }
  
  h2 {
    margin-bottom: var(--spacing-1);
  }
  
  .subtitle {
    margin-bottom: var(--spacing-6);
    color: var(--color-text);
    opacity: 0.8;
  }
  
  .form-group {
    margin-bottom: var(--spacing-4);
  }
  
  label {
    display: block;
    margin-bottom: var(--spacing-2);
    font-weight: 700;
  }
  
  input {
    width: 100%;
    padding: var(--spacing-3);
    border: 2px solid var(--color-text);
    border-radius: var(--radius-sm);
    font-size: var(--font-size-sm);
    transition: all 0.2s;
  }
  
  input:focus {
    outline: none;
    border-color: var(--color-secondary);
    box-shadow: 0 0 0 3px rgba(0, 188, 255, 0.3);
  }
  
  .btn {
    width: 100%;
    padding: var(--spacing-3) var(--spacing-4);
    font-weight: 800;
    font-size: var(--font-size-base);
    border: 2px solid var(--color-text);
    border-radius: var(--radius-sm);
    cursor: pointer;
    box-shadow: 4px 4px 0px var(--color-text);
    transition: transform 0.1s, box-shadow 0.1s;
    margin-top: var(--spacing-4);
  }
  
  .btn:active {
    transform: translate(4px, 4px);
    box-shadow: 0px 0px 0px var(--color-text);
  }
  
  .btn:focus-visible {
    outline: 3px dashed var(--color-secondary);
    outline-offset: 2px;
  }
  
  .btn.primary {
    background: var(--color-primary);
    color: var(--color-text);
  }
  
  .alert.danger {
    background: var(--color-danger);
    color: var(--color-surface);
    padding: var(--spacing-3);
    margin-bottom: var(--spacing-4);
    border-radius: var(--radius-sm);
    font-weight: 600;
    border: 2px solid var(--color-text);
  }
</style>
