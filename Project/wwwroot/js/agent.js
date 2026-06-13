// agent.js — wire the frontend terminal to POST /api/chat

const API_BASE = window.location.origin; // same-origin when served by ASP.NET

// Called by the terminal UI when the user hits Enter or clicks Send.
async function callAgentAPI(userText, provider, model) {
  const res = await fetch(`${API_BASE}/api/chat`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ message: userText, provider, model })
  });

  if (!res.ok) {
    const err = await res.json().catch(() => ({ error: `HTTP ${res.status}` }));
    throw new Error(err.error || 'Request failed');
  }

  return await res.json();
}

// Export for use in the main UI script
window.callAgentAPI = callAgentAPI;
