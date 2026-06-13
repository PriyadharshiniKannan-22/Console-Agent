// tokenStats.js — cost calculation for Groq models
// Pricing in USD per 1M tokens (https://console.groq.com/docs/models)

const PRICING = {
  'groq': {
    'llama-3.3-70b-versatile':              { input: 0.59,  output: 0.79  },
    'llama-3.1-8b-instant':                 { input: 0.05,  output: 0.08  },
    'llama-3.3-70b-specdec':                { input: 0.59,  output: 0.99  },
    'meta-llama/llama-4-scout-17b-16e-instruct': { input: 0.11, output: 0.34 },
    'meta-llama/llama-4-maverick-17b-128e-instruct': { input: 0.20, output: 0.60 },
    'gemma2-9b-it':                         { input: 0.20,  output: 0.20  },
    'mixtral-8x7b-32768':                   { input: 0.24,  output: 0.24  },
  }
};

/**
 * Calculate estimated cost for a single turn.
 * @param {string} provider - always 'groq'
 * @param {string} model    - groq model string
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
 * Shows more decimal places for sub-cent amounts.
 */
function fmtCost(usd) {
  if (usd === 0) return '$0.0000';
  if (usd < 0.0001) return '$' + usd.toFixed(8);
  if (usd < 0.01)   return '$' + usd.toFixed(6);
  return '$' + usd.toFixed(4);
}

window.calcCost = calcCost;
window.fmtCost  = fmtCost;
