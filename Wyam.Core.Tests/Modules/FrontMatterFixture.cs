﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Wyam.Core.Modules;
using Wyam.Abstractions;

namespace Wyam.Core.Tests.Modules
{
    [TestFixture]
    public class FrontMatterFixture
    {
        [Test]
        public void DefaultCtorSplitsAtDashes()
        {
            // Given
            Engine engine = new Engine();
            Metadata metadata = new Metadata(engine);
            IPipelineContext pipeline = new Pipeline(engine, null, null);
            IDocument[] inputs = { new Document(metadata).Clone(@"FM1
FM2
---
Content1
Content2") };
            string frontMatterContent = null;
            FrontMatter frontMatter = new FrontMatter(new Execute(x =>
            {
                frontMatterContent = x.Content;
                return new [] {x};
            }));

            // When
            IEnumerable<IDocument> documents = frontMatter.Execute(inputs, pipeline);

            // Then
            Assert.AreEqual(1, documents.Count());
            Assert.AreEqual(@"FM1
FM2
", frontMatterContent);
            Assert.AreEqual(@"Content1
Content2", documents.First().Content);
        }

        [Test]
        public void DashStringDoesNotSplitAtNonmatchingDashes()
        {
            // Given
            Engine engine = new Engine();
            Metadata metadata = new Metadata(engine);
            IPipelineContext pipeline = new Pipeline(engine, null, null);
            IDocument[] inputs = { new Document(metadata).Clone(@"FM1
FM2
---
Content1
Content2") };
            bool executed = false;
            FrontMatter frontMatter = new FrontMatter("-", new Execute(x =>
            {
                executed = true;
                return new[] { x };
            }));

            // When
            IEnumerable<IDocument> documents = frontMatter.Execute(inputs, pipeline);

            // Then
            Assert.AreEqual(1, documents.Count());
            Assert.IsFalse(executed);
            Assert.AreEqual(@"FM1
FM2
---
Content1
Content2", documents.First().Content);
        }

        [Test]
        public void MatchingStringSplitsAtCorrectLocation()
        {
            // Given
            Engine engine = new Engine();
            Metadata metadata = new Metadata(engine);
            IPipelineContext pipeline = new Pipeline(engine, null, null);
            IDocument[] inputs = { new Document(metadata).Clone(@"FM1
FM2
ABC
Content1
Content2") };
            string frontMatterContent = null;
            FrontMatter frontMatter = new FrontMatter("ABC", new Execute(x =>
            {
                frontMatterContent = x.Content;
                return new[] { x };
            }));

            // When
            IEnumerable<IDocument> documents = frontMatter.Execute(inputs, pipeline);

            // Then
            Assert.AreEqual(1, documents.Count());
            Assert.AreEqual(@"FM1
FM2
", frontMatterContent);
            Assert.AreEqual(@"Content1
Content2", documents.First().Content);
        }

        [Test]
        public void SingleCharWithRepeatedDelimiterSplitsAtCorrectLocation()
        {
            // Given
            Engine engine = new Engine();
            Metadata metadata = new Metadata(engine);
            IPipelineContext pipeline = new Pipeline(engine, null, null);
            IDocument[] inputs = { new Document(metadata).Clone(@"FM1
FM2
!!!!
Content1
Content2") };
            string frontMatterContent = null;
            FrontMatter frontMatter = new FrontMatter('!', new Execute(x =>
            {
                frontMatterContent = x.Content;
                return new[] { x };
            }));

            // When
            IEnumerable<IDocument> documents = frontMatter.Execute(inputs, pipeline);

            // Then
            Assert.AreEqual(1, documents.Count());
            Assert.AreEqual(@"FM1
FM2
", frontMatterContent);
            Assert.AreEqual(@"Content1
Content2", documents.First().Content);
        }

        [Test]
        public void SingleCharWithRepeatedDelimiterWithTrailingSpacesSplitsAtCorrectLocation()
        {
            // Given
            Engine engine = new Engine();
            Metadata metadata = new Metadata(engine);
            IPipelineContext pipeline = new Pipeline(engine, null, null);
            IDocument[] inputs = { new Document(metadata).Clone(@"FM1
FM2
!!!!  
Content1
Content2") };
            string frontMatterContent = null;
            FrontMatter frontMatter = new FrontMatter('!', new Execute(x =>
            {
                frontMatterContent = x.Content;
                return new[] { x };
            }));

            // When
            IEnumerable<IDocument> documents = frontMatter.Execute(inputs, pipeline);

            // Then
            Assert.AreEqual(1, documents.Count());
            Assert.AreEqual(@"FM1
FM2
", frontMatterContent);
            Assert.AreEqual(@"Content1
Content2", documents.First().Content);
        }

        [Test]
        public void SingleCharWithRepeatedDelimiterWithLeadingSpacesDoesNotSplit()
        {
            // Given
            Engine engine = new Engine();
            Metadata metadata = new Metadata(engine);
            IPipelineContext pipeline = new Pipeline(engine, null, null);
            IDocument[] inputs = { new Document(metadata).Clone(@"FM1
FM2
  !!!!
Content1
Content2") };
            bool executed = false;
            FrontMatter frontMatter = new FrontMatter('!', new Execute(x =>
            {
                executed = true;
                return new[] { x };
            }));

            // When
            IEnumerable<IDocument> documents = frontMatter.Execute(inputs, pipeline);

            // Then
            Assert.AreEqual(1, documents.Count());
            Assert.IsFalse(executed);
            Assert.AreEqual(@"FM1
FM2
  !!!!
Content1
Content2", documents.First().Content);
        }

        [Test]
        public void SingleCharWithRepeatedDelimiterWithExtraLinesSplitsAtCorrectLocation()
        {
            // Given
            Engine engine = new Engine();
            Metadata metadata = new Metadata(engine);
            IPipelineContext pipeline = new Pipeline(engine, null, null);
            IDocument[] inputs = { new Document(metadata).Clone(@"FM1
FM2

!!!!

Content1
Content2") };
            string frontMatterContent = null;
            FrontMatter frontMatter = new FrontMatter('!', new Execute(x =>
            {
                frontMatterContent = x.Content;
                return new[] { x };
            }));

            // When
            IEnumerable<IDocument> documents = frontMatter.Execute(inputs, pipeline);

            // Then
            Assert.AreEqual(1, documents.Count());
            Assert.AreEqual(@"FM1
FM2

", frontMatterContent);
            Assert.AreEqual(@"
Content1
Content2", documents.First().Content);
        }

        [Test]
        public void SingleCharWithSingleDelimiterSplitsAtCorrectLocation()
        {
            // Given
            Engine engine = new Engine();
            Metadata metadata = new Metadata(engine);
            IPipelineContext pipeline = new Pipeline(engine, null, null);
            IDocument[] inputs = { new Document(metadata).Clone(@"FM1
FM2
!
Content1
Content2") };
            string frontMatterContent = null;
            FrontMatter frontMatter = new FrontMatter('!', new Execute(x =>
            {
                frontMatterContent = x.Content;
                return new[] { x };
            }));

            // When
            IEnumerable<IDocument> documents = frontMatter.Execute(inputs, pipeline);

            // Then
            Assert.AreEqual(1, documents.Count());
            Assert.AreEqual(@"FM1
FM2
", frontMatterContent);
            Assert.AreEqual(@"Content1
Content2", documents.First().Content);
        }

        [Test]
        public void MultipleInputDocumentsResultsInMultipleOutputs()
        {
            // Given
            Engine engine = new Engine();
            Metadata metadata = new Metadata(engine);
            IPipelineContext pipeline = new Pipeline(engine, null, null);
            IDocument[] inputs = { new Document(metadata).Clone(@"AA
-
XX"), new Document(metadata).Clone(@"BB
-
YY") };
            string frontMatterContent = string.Empty;
            FrontMatter frontMatter = new FrontMatter(new Execute(x =>
            {
                frontMatterContent += x.Content;
                return new[] { x };
            }));

            // When
            IEnumerable<IDocument> documents = frontMatter.Execute(inputs, pipeline);

            // Then
            Assert.AreEqual(2, documents.Count());
            Assert.AreEqual(@"AA
BB
", frontMatterContent);
            Assert.AreEqual(@"XX", documents.First().Content);
            Assert.AreEqual(@"YY", documents.Skip(1).First().Content);
        }
    }
}
