// tokenStats.js — cost calculation and token UI updates

// Pricing in USD per 1M tokens (update these as needed)
const PRICING = {
  'anthropic': {
    'claude-3-5-haiku-20241022':  { input: 0.80,  output: 4.00  },
    'claude-3-5-sonnet-20241022': { input: 3.00,  output: 15.00 },
    'claude-opus-4-6':            { input: 15.00, output: 75.00 },
  },
  'openai': {
    'gpt-4o-mini':  { input: 0.15, output: 0.60  },
    'gpt-4o':       { input: 2.50, output: 10.00 },
    'o1-mini':      { input: 1.10, output: 4.40  },
  }
};

/**
 * Calculate estimated cost for a single turn.
 * @param {string} provider - 'anthropic' | 'openai'
 * @param {string} model    - model string
 * @param {number} inTok    - input tokens used
 * @param {number} outTok   - output tokens used
 * @returns {number} cost in USD
 */
function calcCost(provider, model, inTok, outTok) {
  const rates = PRICING[provider]?.[model] ?? { input: 0, output: 0 };
  return (inTok / 1_000_000) * rates.input + (outTok / 1_000_000) * rates.output;
}

/**
 * Format a cost number as a $ string.
 * Shows 4 decimal places for sub-cent amounts.
 */
function fmtCost(usd) {
  if (usd === 0) return '$0.0000';
  if (usd < 0.01) return '$' + usd.toFixed(6);
  return '$' + usd.toFixed(4);
}

window.calcCost = calcCost;
window.fmtCost  = fmtCost;
