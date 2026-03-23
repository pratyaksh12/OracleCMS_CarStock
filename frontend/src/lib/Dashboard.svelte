<script lang="ts">
  import { createEventDispatcher, onMount } from 'svelte';
  
  let { token } = $props();
  const dispatch = createEventDispatcher();
  
  let cars: any[] = $state([]);
  let error = $state('');
  let newCar: Record<string, any> = $state({ Make: '', Model: '', Year: new Date().getFullYear(), StockLevel: 1 });
  let searchMake = $state('');
  let searchModel = $state('');
  
  const API_BASE = 'http://localhost:5237';
  
  async function fetchCars() {
    try {
      let url = `${API_BASE}/cars`;
      if (searchMake.trim() || searchModel.trim()) {
        const makeQuery = encodeURIComponent(searchMake.trim());
        const modelQuery = encodeURIComponent(searchModel.trim());
        url = `${API_BASE}/cars/search?Make=${makeQuery}&Model=${modelQuery}`;
      }
      
      const res = await fetch(url, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      
      if (!res.ok) throw new Error('Failed to fetch cars');
      
      const resData = await res.json();
      cars = resData.data || [];
    } catch (err: any) {
      error = err.message;
    }
  }
  
  async function addCar(e: Event) {
    e.preventDefault();
    try {
      const res = await fetch(`${API_BASE}/cars`, {
        method: 'POST',
        headers: { 
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}` 
        },
        body: JSON.stringify(newCar)
      });
      
      if (!res.ok) throw new Error('Failed to add car');
      
      newCar = { Make: '', Model: '', Year: new Date().getFullYear(), StockLevel: 1 };
      await fetchCars();
    } catch (err: any) {
      error = err.message;
    }
  }
  
  async function updateStock(carId: number, currentStock: number, change: number) {
    const newStock = currentStock + change;
    if (newStock < 0) return;
    
    try {
      const res = await fetch(`${API_BASE}/cars/${carId}/stock`, {
        method: 'PATCH',
        headers: { 
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}` 
        },
        body: JSON.stringify({ CarId: carId, StockLevel: newStock })
      });
      
      if (!res.ok) throw new Error('Failed to update stock');
      await fetchCars();
    } catch (err: any) {
      error = err.message;
    }
  }
  
  async function deleteCar(carId: number) {
    if (!confirm('Are you sure you want to delete this car?')) return;
    
    try {
      const res = await fetch(`${API_BASE}/car/${carId}`, {
        method: 'DELETE',
        headers: { 
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}` 
        },
        body: JSON.stringify({ CarId: carId })
      });
      
      if (!res.ok) throw new Error('Failed to delete car');
      await fetchCars();
    } catch (err: any) {
      error = err.message;
    }
  }
  
  onMount(fetchCars);
</script>

<div class="dashboard">
  <header>
    <h2>Inventory Dashboard</h2>
    <button class="btn logout" onclick={() => dispatch('logout')}>Logout</button>
  </header>

  {#if error}
    <div class="alert danger">{error}</div>
  {/if}

  <section class="card action-section">
    <h3>Add New Car</h3>
    <form class="inline-form" onsubmit={addCar}>
      <input bind:value={newCar.Make} placeholder="Make (e.g. Toyota)" required />
      <input bind:value={newCar.Model} placeholder="Model (e.g. Corolla)" required />
      <input type="number" bind:value={newCar.Year} placeholder="Year" required />
      <input type="number" bind:value={newCar.StockLevel} placeholder="Stock" min="0" required />
      <button type="submit" class="btn primary">Add</button>
    </form>
  </section>

  <section class="card">
    <div class="table-header">
      <h3>Current Stock</h3>
      <div class="search-bar">
        <input bind:value={searchMake} placeholder="Search by Make..." />
        <input bind:value={searchModel} placeholder="Search by Model..." />
        <button class="btn secondary" onclick={fetchCars}>Search</button>
        {#if searchMake || searchModel}
          <button class="btn" onclick={() => { searchMake = ''; searchModel = ''; fetchCars(); }}>Clear</button>
        {/if}
      </div>
    </div>

    {#if cars.length === 0}
      <div class="empty-state">
        <p>No cars found in your inventory.</p>
      </div>
    {:else}
      <table>
        <thead>
          <tr>
            <th>Make</th>
            <th>Model</th>
            <th>Year</th>
            <th>Stock Level</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {#each cars as car}
            <tr>
              <td><strong>{car.make}</strong></td>
              <td>{car.model}</td>
              <td>{car.year}</td>
              <td>
                <div class="stock-control">
                  <button class="btn icon" onclick={() => updateStock(car.carId, car.stockLevel, -1)} disabled={car.stockLevel <= 0}>-</button>
                  <span class="stock-badge {car.stockLevel === 0 ? 'out-of-stock' : 'in-stock'}">
                    {car.stockLevel}
                  </span>
                  <button class="btn icon" onclick={() => updateStock(car.carId, car.stockLevel, 1)}>+</button>
                </div>
              </td>
              <td>
                <button class="btn danger" onclick={() => deleteCar(car.carId)}>Delete</button>
              </td>
            </tr>
          {/each}
        </tbody>
      </table>
    {/if}
  </section>
</div>

<style>
  header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: var(--spacing-6);
  }
  
  .card {
    background: var(--color-surface);
    border: 3px solid var(--color-text);
    border-radius: var(--radius-base);
    box-shadow: 6px 6px 0px var(--color-text);
    padding: var(--spacing-6);
    margin-bottom: var(--spacing-6);
  }
  
  .inline-form {
    display: flex;
    gap: var(--spacing-3);
    flex-wrap: wrap;
    align-items: center;
  }
  
  input {
    flex: 1;
    min-width: 120px;
    padding: var(--spacing-3);
    border: 2px solid var(--color-text);
    border-radius: var(--radius-sm);
  }
  
  .table-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: var(--spacing-4);
    flex-wrap: wrap;
    gap: var(--spacing-4);
  }
  
  .search-bar {
    display: flex;
    gap: var(--spacing-2);
  }
  
  .btn {
    padding: var(--spacing-2) var(--spacing-4);
    font-weight: 700;
    border: 2px solid var(--color-text);
    border-radius: var(--radius-sm);
    cursor: pointer;
    box-shadow: 3px 3px 0px var(--color-text);
    transition: transform 0.1s, box-shadow 0.1s;
    background: var(--color-surface);
  }
  
  .btn:active:not(:disabled) {
    transform: translate(3px, 3px);
    box-shadow: 0px 0px 0px var(--color-text);
  }
  
  .btn:disabled {
    opacity: 0.5;
    cursor: not-allowed;
    box-shadow: none;
  }
  
  .btn.primary {
    background: var(--color-primary);
  }
  
  .btn.secondary {
    background: var(--color-secondary);
  }
  
  .btn.danger {
    background: var(--color-danger);
    color: var(--color-surface);
  }
  
  .btn.icon {
    padding: var(--spacing-1) var(--spacing-2);
  }
  
  .stock-control {
    display: flex;
    align-items: center;
    gap: var(--spacing-3);
  }
  
  .stock-badge {
    font-weight: 800;
    font-family: var(--font-mono);
    padding: var(--spacing-1) var(--spacing-3);
    border: 2px solid var(--color-text);
    border-radius: var(--radius-sm);
  }
  
  .in-stock {
    background: var(--color-primary);
  }
  
  .out-of-stock {
    background: var(--color-warning);
    color: var(--color-surface);
  }

  .empty-state {
    text-align: center;
    padding: var(--spacing-8);
    background: var(--color-bg);
    border: 2px dashed var(--color-text);
    border-radius: var(--radius-base);
  }
</style>
