import { defineCollection } from 'astro:content';
import { docsLoader } from '@astrojs/starlight/loaders';
import { docsSchema } from '@astrojs/starlight/schema';
import { changelogsLoader } from 'starlight-changelogs/loader';

export const collections = {
    docs: defineCollection({ loader: docsLoader(), schema: docsSchema() }),
    changelogs: defineCollection({
        loader: changelogsLoader([
            {
                provider: 'keep-a-changelog',
                base: 'changelog/fable-ink',
                changelog: '../src/Fable.Ink/CHANGELOG.md'
            },
            {
                provider: 'keep-a-changelog',
                base: 'changelog/fable-parchment',
                changelog: '../src/Fable.Parchment/CHANGELOG.md'
            },
            {
                provider: 'keep-a-changelog',
                base: 'changelog/fable-nib',
                changelog: '../src/Fable.Nib/CHANGELOG.md'
            },
            {
                provider: 'keep-a-changelog',
                base: 'changelog/fable-nib-snapshot',
                changelog: '../src/Fable.Nib.Snapshot/CHANGELOG.md'
            },
            {
                provider: 'keep-a-changelog',
                base: 'changelog/fable-nib-browser',
                changelog: '../src/Fable.Nib.Browser/CHANGELOG.md'
            },
            {
                provider: 'keep-a-changelog',
                base: 'changelog/fable-quill',
                changelog: '../src/Fable.Quill/CHANGELOG.md'
            },
        ]),
    })
};
