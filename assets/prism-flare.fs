/*{
  "DESCRIPTION": "A rotating spectral flare sweeping from a fixed hotspot — a holographic prism streak that continuously rotates, so every second of footage catches a different rainbow glint automatically.",
  "CATEGORIES": ["Guillotine", "Stylize"],
  "INPUTS": [
    { "NAME": "inputImage", "TYPE": "image" },
    { "NAME": "intensity", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.05, "MAX": 1.5 }
  ]
}*/

// A cheap rainbow ramp from three phase-shifted cosines (the classic Inigo Quilez palette trick).
vec3 spectrum(float t) {
  return 0.5 + 0.5 * cos(6.28318 * (t + vec3(0.0, 0.33, 0.67)));
}

void main() {
  vec2 uv = isf_FragNormCoord;
  vec2 hotspot = vec2(0.85, 0.9);
  vec2 toPixel = uv - hotspot;
  float angle = atan(toPixel.y, toPixel.x);

  float sweep = mod(TIME * speed, 6.28318);
  float diff = abs(mod(angle - sweep + 3.14159, 6.28318) - 3.14159);

  // A soft angular band around the sweep line, tighter near the hotspot, softer far away.
  float dist = length(toPixel);
  float band = smoothstep(0.55, 0.0, diff) * smoothstep(1.1, 0.15, dist);

  vec4 base = IMG_THIS_PIXEL(inputImage);
  vec3 flare = spectrum(dist * 1.5 - TIME * speed * 0.5);
  vec3 result = base.rgb + flare * band * intensity;

  gl_FragColor = vec4(result, base.a);
}
