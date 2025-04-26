// See https://aka.ms/new-console-template for more information
using _04_TPL_Library;

//_01_BufferBlock bufferBlock = new _01_BufferBlock();
//bufferBlock.RunBuffer().GetAwaiter().GetResult();

//_02_BroadcastBlock broadcastBlock = new _02_BroadcastBlock();
//broadcastBlock.RunBroadcastBlock().GetAwaiter().GetResult(); 

//_03_ActionBlog actionBlog = new _03_ActionBlog();
//actionBlog.RunActionBlock().GetAwaiter().GetResult();

_04_TransformBlock transformBlock = new _04_TransformBlock();
transformBlock.RunTransform().GetAwaiter().GetResult();
