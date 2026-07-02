// @ts-check
import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';
import fsharpOracle from 'starlight-fsharp-oracle';
import starlightChangelogs, { makeChangelogsSidebarLinks } from 'starlight-changelogs';
import fsharpLiterate from 'starlight-fsharp-literate';
import starlightLinksValidator from 'starlight-links-validator'

import { fileURLToPath } from 'node:url';
import { resolve, dirname } from 'node:path';

const __dirname = dirname(fileURLToPath(import.meta.url));

// https://astro.build/config
export default defineConfig({
    base: '/Scriptorium/',
    server: { host: true },
    integrations: [
        fsharpLiterate(),
        starlight({
            title: 'Scriptorium',
            social: [{ icon: 'github', label: 'GitHub', href: 'https://github.com/fable-hub/Scriptorium' }],
            customCss: [
                './src/styles/custom.css',
            ],
            plugins: [
                starlightChangelogs(),
                starlightLinksValidator(),
                fsharpOracle({
                    assemblies: [
                        resolve(__dirname, '../src/Scriptorium.Ink/bin/Debug/netstandard2.1/publish/Scriptorium.Ink.dll'),
                        resolve(__dirname, '../src/Scriptorium.Nib/bin/Debug/netstandard2.1/publish/Scriptorium.Nib.dll'),
                        resolve(__dirname, '../src/Scriptorium.Nib.Browser/bin/Debug/netstandard2.1/publish/Scriptorium.Nib.Browser.dll'),
                        resolve(__dirname, '../src/Scriptorium.Nib.Snapshot/bin/Debug/net10.0/publish/Scriptorium.Nib.Snapshot.dll'),
                        resolve(__dirname, '../src/Scriptorium.Parchment/bin/Debug/netstandard2.1/publish/Scriptorium.Parchment.dll'),
                        resolve(__dirname, '../src/Scriptorium.Quill/bin/Debug/netstandard2.1/publish/Scriptorium.Quill.dll'),
                    ],
                    output: 'api',
                    sidebar: { label: 'API Reference' }
                }),
            ],
            sidebar: [
                {
                    label: 'Introduction',
                    items: [
                        'guides/why',
                        'guides/story',
                        'guides/getting-started'
                    ],
                },
                {
                    label: 'Scriptorium.Ink',
                    items: [
                        'ink/overview'
                    ],
                },
                {
                    label: 'Scriptorium.Parchment',
                    items: [
                        'parchment/overview',
                    ],
                },
                {
                    label: 'Scriptorium.Nib',
                    items: [
                        'nib/overview',
                        'nib/combinators',
                        'nib/assertions',
                        'nib/extending'
                    ],
                },
                {
                    label: 'Scriptorium.Quill',
                    items: [
                        'quill/overview',
                        'quill/test-dsl',
                        'quill/configuration',
                        'quill/runner',
                    ],
                },
                {
                    label: 'Scriptorium.Hedgehog',
                    items: [
                        'hedgehog/overview',
                    ],
                },
                {
                    label: 'Scriptorium.Nib.Browser',
                    items: [
                        'nib-browser/overview',
                        'nib-browser/assertions',
                        'nib-browser/browser-tests',
                    ],
                },
                {
                    label: 'Scriptorium.Nib.Snapshot',
                    items: [
                        'nib-snapshot/overview',
                        'nib-snapshot/configuration',
                    ],
                },
                {
                    label: 'Changelogs',
                    collapsed: true, // Make the section less intrusive
                    items: [
                        ...makeChangelogsSidebarLinks([
                            { type: 'all', base: 'changelog/scriptorium-ink', label: 'Scriptorium.Ink' },
                            { type: 'all', base: 'changelog/scriptorium-parchment', label: 'Scriptorium.Parchment' },
                            { type: 'all', base: 'changelog/scriptorium-nib', label: 'Scriptorium.Nib' },
                            { type: 'all', base: 'changelog/scriptorium-nib-snapshot', label: 'Scriptorium.Nib.Snapshot' },
                            { type: 'all', base: 'changelog/scriptorium-nib-browser', label: 'Scriptorium.Nib.Browser' },
                            { type: 'all', base: 'changelog/scriptorium-quill', label: 'Scriptorium.Quill' },
                        ])
                    ]
                }
            ],
        }),
    ],
});
